import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from './notification.service';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred';

      if (error.status === 0) {
        errorMessage = `Unable to connect to server: ${req.url}`;
      } else if (error.status >= 400 && error.status < 500) {
        errorMessage = error.error?.message || error.message || `Request failed: ${error.status}`;
      } else if (error.status >= 500) {
        errorMessage = `Server error: ${error.status} ${error.statusText}`;
      }

      notificationService.showError(errorMessage, error);

      return throwError(() => error);
    })
  );
};
