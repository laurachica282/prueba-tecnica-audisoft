import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'students', pathMatch: 'full' },
    { path: '**', redirectTo: 'students' }
];
