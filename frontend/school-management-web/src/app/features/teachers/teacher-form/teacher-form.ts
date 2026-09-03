import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Teacher } from '../../../core/models/teacher.model';
import { TeacherService } from '../../../core/services/teacher.service';

@Component({
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressBarModule
  ],
  selector: 'app-teacher-form',
  styleUrl: './teacher-form.scss',
  templateUrl: './teacher-form.html',
})
export class TeacherForm {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(TeacherService);
  private readonly dialogRef = inject(MatDialogRef<TeacherForm>);
  readonly data = inject<Teacher | null>(MAT_DIALOG_DATA);

  readonly isEditMode = this.data !== null;
  readonly saving = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: [
      this.data?.name ?? '',
      [Validators.required, Validators.minLength(3), Validators.maxLength(100)]
    ]
  });

  get nameControl() {
    return this.form.controls.name;
  }

  nameError(): string {
    const control = this.nameControl;
    if (control.hasError('required')) return 'El nombre es obligatorio.';
    if (control.hasError('minlength')) return 'Debe tener al menos 3 caracteres.';
    if (control.hasError('maxlength')) return 'No puede superar los 100 caracteres.';
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
    const payload = { name: this.form.getRawValue().name.trim() };

    const request$ = this.isEditMode
      ? this.service.update(this.data!.id, payload)
      : this.service.create(payload);

    request$.subscribe({
      next: () => this.dialogRef.close(true),
      error: () => this.saving.set(false)
    });
  }
}
