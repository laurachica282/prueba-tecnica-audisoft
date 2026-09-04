import { Injectable, computed, inject, signal } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from './auth.config';

export type AppRole = 'administrator' | 'teacher' | 'student';

interface UserProfile {
  username: string;
  fullName: string;
  email: string;
  roles: AppRole[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly oauth = inject(OAuthService);

  private readonly profile = signal<UserProfile | null>(null);

  readonly currentUser = this.profile.asReadonly();

  readonly isAuthenticated = computed(() => this.profile() !== null);

  readonly isAdministrator = computed(() =>
    this.profile()?.roles.includes('administrator') ?? false
  );

  readonly isTeacher = computed(() =>
    this.profile()?.roles.includes('teacher') ?? false
  );

  readonly isStudent = computed(() =>
    this.profile()?.roles.includes('student') ?? false
  );

  readonly roleLabel = computed(() => {
    const user = this.profile();
    if (!user) return '';
    if (user.roles.includes('administrator')) return 'Administrador';
    if (user.roles.includes('teacher')) return 'Profesor';
    if (user.roles.includes('student')) return 'Estudiante';
    return 'Sin rol asignado';
  });

  async initialize(): Promise<void> {
    this.oauth.configure(authConfig);
    this.oauth.setupAutomaticSilentRefresh();

    await this.oauth.loadDiscoveryDocumentAndTryLogin();

    if (this.oauth.hasValidAccessToken()) {
      this.loadProfile();
    }
  }

  login(): void {
    this.oauth.initCodeFlow();
  }

  logout(): void {
    this.profile.set(null);
    this.oauth.logOut();
  }

  getAccessToken(): string | null {
    return this.oauth.getAccessToken();
  }

  hasValidToken(): boolean {
    return this.oauth.hasValidAccessToken();
  }

  private loadProfile(): void {
    const claims = this.oauth.getIdentityClaims() as Record<string, unknown> | null;
    if (!claims) return;

    const token = this.decodeAccessToken();
    const realmAccess = token?.['realm_access'] as { roles?: string[] } | undefined;

    const knownRoles: AppRole[] = ['administrator', 'teacher', 'student'];

    const roles = (realmAccess?.roles ?? [])
      .filter((role): role is AppRole => knownRoles.includes(role as AppRole));

    this.profile.set({
      username: (claims['preferred_username'] as string) ?? '',
      fullName: (claims['name'] as string) ?? '',
      email: (claims['email'] as string) ?? '',
      roles
    });
  }

  private decodeAccessToken(): Record<string, unknown> | null {
    const token = this.oauth.getAccessToken();
    if (!token) return null;

    try {
      const payload = token.split('.')[1];
      const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
      return JSON.parse(atob(normalized));
    } catch {
      return null;
    }
  }
}