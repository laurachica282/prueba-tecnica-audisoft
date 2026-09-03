import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'students', pathMatch: 'full' },
    {
        path: 'students',
        loadComponent: () =>
            import('./features/students/student-list/student-list').then((m) => m.StudentList),
        title: 'Estudiantes'
    },
    { path: '**', redirectTo: 'students' }
];
