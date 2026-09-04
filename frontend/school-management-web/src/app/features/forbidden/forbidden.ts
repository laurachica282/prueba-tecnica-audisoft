import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  imports: [MatButtonModule, MatIconModule],
  selector: 'app-forbidden',
  styleUrl: './forbidden.scss',
  templateUrl: './forbidden.html',
})
export class Forbidden {
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);

  goBack(): void {
    this.router.navigate(['/students']);
  }
}
