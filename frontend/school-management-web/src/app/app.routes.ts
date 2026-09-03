import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'students', pathMatch: 'full' },
    {
        path: 'students',
        loadComponent: () =>
            import('./features/students/student-list/student-list').then((m) => m.StudentList),
        title: 'Estudiantes'
    },
    {
        path: 'teachers',
        loadComponent: () =>
            import('./features/teachers/teacher-list/teacher-list').then((m) => m.TeacherList),
        title: 'Profesores'
    },
    {
        path: 'grades',
        loadComponent: () =>
            import('./features/grades/grade-list/grade-list').then((m) => m.GradeList),
        title: 'Notas'
    },
    { path: '**', redirectTo: 'students' }
];
