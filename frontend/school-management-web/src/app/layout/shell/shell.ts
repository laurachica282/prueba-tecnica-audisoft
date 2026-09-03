import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-shell',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule
  ],
  styleUrl: './shell.scss',
  templateUrl: './shell.html'
})
export class Shell {
  readonly navItems = [
    { path: '/students', label: 'Estudiantes', icon: 'school' },
    { path: '/teachers', label: 'Profesores', icon: 'person' },
    { path: '/grades', label: 'Notas', icon: 'grading' }
  ];
}
