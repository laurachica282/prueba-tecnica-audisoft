import { Component, OnInit, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { Student } from '../../../core/models/student.model';
import { NotificationService } from '../../../core/services/notification.service';
import { StudentService } from '../../../core/services/student.service';
import { ConfirmDialog, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import { StudentForm } from '../student-form/student-form';
import { MatSortModule, Sort } from '@angular/material/sort';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatSortModule
  ],
  selector: 'app-student-list',
  styleUrl: './student-list.scss',
  templateUrl: './student-list.html',
})
export class StudentList implements OnInit {
  private readonly service = inject(StudentService);
  private readonly dialog = inject(MatDialog);
  private readonly notification = inject(NotificationService);

  readonly pageSizeOptions = [5, 10, 25];

  readonly students = signal<Student[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);

  readonly auth = inject(AuthService);

  readonly searchControl = new FormControl('', { nonNullable: true });

  private page = 1;
  private pageSize = 5;
  private sortBy = 'name';
  private sortDirection: 'asc' | 'desc' = 'asc';

  ngOnInit(): void {
    this.load();

    this.searchControl.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged())
      .subscribe(() => {
        this.page = 1;
        this.load();
      });
  }

  get activeSort(): string {
    return this.sortBy;
  }

  get sortDir(): 'asc' | 'desc' {
    return this.sortDirection;
  }

  onSortChange(sort: Sort): void {
    this.sortBy = sort.direction ? sort.active : 'name';
    this.sortDirection = (sort.direction || 'asc') as 'asc' | 'desc';
    this.page = 1;
    this.load();
  }

  load(): void {
    this.loading.set(true);

    this.service
      .getPaged({
        page: this.page,
        pageSize: this.pageSize,
        search: this.searchControl.value,
        sortBy: this.sortBy,
        sortDirection: this.sortDirection
      })
      .subscribe({
        next: (result) => {
          this.students.set(result.items);
          this.totalCount.set(result.totalCount);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  onPageChange(event: PageEvent): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }

  get pageIndex(): number {
    return this.page - 1;
  }

  openCreate(): void {
    const ref = this.dialog.open(StudentForm, {
      data: null,
      width: '460px',
      disableClose: true
    });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.notification.success('Estudiante creado exitosamente.');
        this.page = 1;
        this.load();
      }
    });
  }

  openEdit(student: Student): void {
    const ref = this.dialog.open(StudentForm, {
      data: student,
      width: '460px',
      disableClose: true
    });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.notification.success('Estudiante actualizado exitosamente.');
        this.load();
      }
    });
  }

  confirmDelete(student: Student): void {
    const data: ConfirmDialogData = {
      title: 'Eliminar estudiante',
      message: `¿Está seguro de eliminar a "${student.name}"? Esta acción no se puede deshacer.`
    };

    const ref = this.dialog.open(ConfirmDialog, { data, width: '440px' });

    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) this.remove(student);
    });
  }

  private remove(student: Student): void {
    this.loading.set(true);

    this.service.delete(student.id).subscribe({
      next: () => {
        this.notification.success('Estudiante eliminado exitosamente.');

        if (this.students().length === 1 && this.page > 1) {
          this.page--;
        }
        this.load();
      },
      error: () => this.loading.set(false)
    });
  }

  coveragePercent(student: Student): number {
    if (student.totalTeachers === 0) return 0;
    return (student.distinctTeacherCount / student.totalTeachers) * 100;
  }

  coverageClass(student: Student): string {
    const percent = this.coveragePercent(student);
    if (percent === 0) return 'coverage--empty';
    if (percent >= 100) return 'coverage--full';
    return 'coverage--partial';
  }

  get displayedColumns(): string[] {
  const base = ['id', 'name', 'coverage', 'gradeCount'];
  return this.auth.isAdministrator() ? [...base, 'actions'] : base;
}
}
