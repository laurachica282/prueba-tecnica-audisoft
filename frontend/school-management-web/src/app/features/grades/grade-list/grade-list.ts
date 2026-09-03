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
import { Grade } from '../../../core/models/grade.model';
import { GradeService } from '../../../core/services/grade.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmDialogData, ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { GradeForm } from '../grade-form/grade-form';
import { DecimalPipe } from '@angular/common';

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
    MatTooltipModule,
    DecimalPipe
  ],
  selector: 'app-grade-list',
  styleUrl: './grade-list.scss',
  templateUrl: './grade-list.html',
})
export class GradeList implements OnInit {
  private readonly service = inject(GradeService);
  private readonly dialog = inject(MatDialog);
  private readonly notification = inject(NotificationService);

  readonly displayedColumns = ['id', 'name', 'studentName', 'teacherName', 'value', 'actions'];
  readonly pageSizeOptions = [5, 10, 25];

  readonly grades = signal<Grade[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);

  readonly searchControl = new FormControl('', { nonNullable: true });

  private page = 1;
  private pageSize = 5;
  private sortBy = 'studentName';
  private sortDirection: 'asc' | 'desc' = 'asc';

  get pageIndex(): number { return this.page - 1; }
  get activeSort(): string { return this.sortBy; }
  get sortDir(): 'asc' | 'desc' { return this.sortDirection; }

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
          this.grades.set(result.items);
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
    this.sortBy = sort.direction ? sort.active : 'studentName';
    this.sortDirection = (sort.direction || 'asc') as 'asc' | 'desc';
    this.page = 1;
    this.load();
  }

  valueClass(value: number): string {
    if (value < 3) return 'score--low';
    if (value < 4) return 'score--mid';
    return 'score--high';
  }

  openCreate(): void {
    const ref = this.dialog.open(GradeForm, { data: null, width: '500px', disableClose: true });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.notification.success('Nota creada exitosamente.');
        this.page = 1;
        this.load();
      }
    });
  }

  openEdit(grade: Grade): void {
    const ref = this.dialog.open(GradeForm, { data: grade, width: '500px', disableClose: true });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.notification.success('Nota actualizada exitosamente.');
        this.load();
      }
    });
  }

  confirmDelete(grade: Grade): void {
    const data: ConfirmDialogData = {
      title: 'Eliminar nota',
      message: `¿Está seguro de eliminar la nota "${grade.name}" de ${grade.studentName}? Esta acción no se puede deshacer.`
    };

    const ref = this.dialog.open(ConfirmDialog, { data, width: '440px' });

    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) this.remove(grade);
    });
  }

  private remove(grade: Grade): void {
    this.loading.set(true);

    this.service.delete(grade.id).subscribe({
      next: () => {
        this.notification.success('Nota eliminada exitosamente.');

        if (this.grades().length === 1 && this.page > 1) {
          this.page--;
        }
        this.load();
      },
      error: () => this.loading.set(false)
    });
  }
}
