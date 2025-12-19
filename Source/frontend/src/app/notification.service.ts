import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private snackBar = inject(MatSnackBar);

  showError(message: string, ...consoleArgs: unknown[]): void {
    console.error(message, ...consoleArgs);

    const displayMessage = environment.production
      ? 'There was an error. Please retry, refresh, or try again later.'
      : message;

    this.snackBar.open(displayMessage, 'Dismiss', {
      panelClass: ['error-snackbar']
    });
  }
}
