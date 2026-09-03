import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { forkJoin } from 'rxjs';
import { Grade } from '../../../core/models/grade.model';
import { Student } from '../../../core/models/student.model';
import { Teacher } from '../../../core/models/teacher.model';
import { GradeService } from '../../../core/services/grade.service';
import { StudentService } from '../../../core/services/student.service';
import { TeacherService } from '../../../core/services/teacher.service';

@Component({
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressBarModule
  ],
  selector: 'app-grade-form',
  styleUrl: './grade-form.scss',
  templateUrl: './grade-form.html',
})
export class GradeForm {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(GradeService);
  private readonly studentService = inject(StudentService);
  private readonly teacherService = inject(TeacherService);
  private readonly dialogRef = inject(MatDialogRef<GradeForm>);
  readonly data = inject<Grade | null>(MAT_DIALOG_DATA);

  readonly isEditMode = this.data !== null;
  readonly saving = signal(false);
  readonly loadingOptions = signal(true);

  readonly students = signal<Student[]>([]);
  readonly teachers = signal<Teacher[]>([]);

  readonly form = this.fb.nonNullable.group({
    name: [
      this.data?.name ?? '',
      [Validators.required, Validators.minLength(3), Validators.maxLength(100)]
    ],
    value: [
      this.data?.value ?? 0,
      [Validators.required, Validators.min(0), Validators.max(5)]
    ],
    studentId: [this.data?.studentId ?? 0, [Validators.required, Validators.min(1)]],
    teacherId: [this.data?.teacherId ?? 0, [Validators.required, Validators.min(1)]]
  });

  constructor() {
    forkJoin({
      students: this.studentService.getLookup(),
      teachers: this.teacherService.getLookup()
    }).subscribe({
      next: ({ students, teachers }) => {
        this.students.set(students);
        this.teachers.set(teachers);
        this.loadingOptions.set(false);
      },
      error: () => this.loadingOptions.set(false)
    });
  }

  get nameControl() {
    return this.form.controls.name;
  }

  get valueControl() {
    return this.form.controls.value;
  }

  get studentControl() {
    return this.form.controls.studentId;
  }

  get teacherControl() {
    return this.form.controls.teacherId;
  }

  nameError(): string {
    const control = this.nameControl;
    if (control.hasError('required')) return 'El nombre es obligatorio.';
    if (control.hasError('minlength')) return 'Debe tener al menos 3 caracteres.';
    if (control.hasError('maxlength')) return 'No puede superar los 100 caracteres.';
    return '';
  }

  valueError(): string {
    const control = this.valueControl;
    if (control.hasError('required')) return 'La nota es obligatoria.';
    if (control.hasError('min') || control.hasError('max')) {
      return 'La nota debe estar entre 0.0 y 5.0.';
    }
    return '';
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  save(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const raw = this.form.getRawValue();

    const payload = {
      name: raw.name.trim(),
      value: Number(raw.value),
      studentId: raw.studentId,
      teacherId: raw.teacherId
    };

    const request$ = this.isEditMode
      ? this.service.update(this.data!.id, payload)
      : this.service.create(payload);

    request$.subscribe({
      next: () => this.dialogRef.close(true),
      error: () => this.saving.set(false)
    });
  }
}
