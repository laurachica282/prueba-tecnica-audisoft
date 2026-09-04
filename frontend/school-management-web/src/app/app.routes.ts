import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
    { path: '', redirectTo: 'students', pathMatch: 'full' },
    {
        path: 'students',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./features/students/student-list/student-list').then((m) => m.StudentList),
        title: 'Estudiantes'
    },
    {
        path: 'teachers',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./features/teachers/teacher-list/teacher-list').then((m) => m.TeacherList),
        title: 'Profesores'
    },
    {
        path: 'grades',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./features/grades/grade-list/grade-list').then((m) => m.GradeList),
        title: 'Notas'
    },
    {
        path: 'forbidden',
        loadComponent: () =>
            import('./features/forbidden/forbidden').then((m) => m.Forbidden),
        title: 'Acceso restringido'
    },
    { path: '**', redirectTo: 'students' }
];
