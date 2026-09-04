import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      notification.error(buildMessage(error));
      return throwError(() => error);
    })
  );
};

function buildMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'No se pudo conectar con el servidor. Verifique que la API esté en ejecución.';
  }

  if (error.status === 400 && error.error?.errors) {
    const messages = Object.values(error.error.errors).flat() as string[];
    return messages.join(' ');
  }

  if (error.status === 403) {
    return 'No tiene permisos para realizar esta acción.';
  }

  if (error.status === 401) {
    return 'Su sesión expiró. Vuelva a iniciar sesión.';
  }

  if (error.error?.message) {
    return error.error.message;
  }

  return 'Ocurrió un error inesperado. Intente nuevamente.';
}