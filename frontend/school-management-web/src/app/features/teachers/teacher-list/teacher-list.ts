import { Component, inject, OnInit, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { Teacher } from '../../../core/models/teacher.model';
import { NotificationService } from '../../../core/services/notification.service';
import { TeacherService } from '../../../core/services/teacher.service';
import { ConfirmDialogData, ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { TeacherForm } from '../teacher-form/teacher-form';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatTooltipModule
  ],
  selector: 'app-teacher-list',
  styleUrl: './teacher-list.scss',
  templateUrl: './teacher-list.html',
})
export class TeacherList implements OnInit {
  private readonly service = inject(TeacherService);
  private readonly dialog = inject(MatDialog);
  private readonly notification = inject(NotificationService);

  readonly auth = inject(AuthService);
  get displayedColumns(): string[] {
    const base = ['id', 'name', 'coverage', 'gradeCount'];
    return this.auth.isAdministrator() ? [...base, 'actions'] : base;
  }
  readonly pageSizeOptions = [5, 10, 25];

  readonly teachers = signal<Teacher[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);

  readonly searchControl = new FormControl('', { nonNullable: true });

  private page = 1;
  private pageSize = 5;
  private sortBy = 'name';
  private sortDirection: 'asc' | 'desc' = 'asc';

  get pageIndex(): number {
    return this.page - 1;
  }

  get activeSort(): string {
    return this.sortBy;
  }

  get sortDir(): 'asc' | 'desc' {
    return this.sortDirection;
  }

  ngOnInit(): void {
    this.load();

    this.searchControl.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged())
      .subscribe(() => {
        this.page = 1;
        this.load();
      });
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
          this.teachers.set(result.items);
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

  onSortChange(sort: Sort): void {
    this.sortBy = sort.direction ? sort.active : 'name';
    this.sortDirection = (sort.direction || 'asc') as 'asc' | 'desc';
    this.page = 1;
    this.load();
  }

  openCreate(): void {
    const ref = this.dialog.open(TeacherForm, { data: null, width: '460px', disableClose: true });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.notification.success('Profesor creado exitosamente.');
        this.page = 1;
        this.load();
      }
    });
  }

  openEdit(teacher: Teacher): void {
    const ref = this.dialog.open(TeacherForm, { data: teacher, width: '460px', disableClose: true });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.notification.success('Profesor actualizado exitosamente.');
        this.load();
      }
    });
  }

  confirmDelete(teacher: Teacher): void {
    const data: ConfirmDialogData = {
      title: 'Eliminar profesor',
      message: `¿Está seguro de eliminar a "${teacher.name}"? Esta acción no se puede deshacer.`
    };

    const ref = this.dialog.open(ConfirmDialog, { data, width: '440px' });

    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) this.remove(teacher);
    });
  }

  private remove(teacher: Teacher): void {
    this.loading.set(true);

    this.service.delete(teacher.id).subscribe({
      next: () => {
        this.notification.success('Profesor eliminado exitosamente.');

        if (this.teachers().length === 1 && this.page > 1) {
          this.page--;
        }
        this.load();
      },
      error: () => this.loading.set(false)
    });
  }

  coveragePercent(teacher: Teacher): number {
    if (teacher.totalStudents === 0) return 0;
    return (teacher.distinctStudentCount / teacher.totalStudents) * 100;
  }

  coverageClass(teacher: Teacher): string {
    const percent = this.coveragePercent(teacher);
    if (percent === 0) return 'coverage--empty';
    if (percent >= 100) return 'coverage--full';
    return 'coverage--partial';
  }
}
