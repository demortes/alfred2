import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from 'src/environments/environment';
import { BotSettings } from './models/bot-settings';

@Injectable({
    providedIn: 'root'
})
export class BotSettingsService {
    private apiUrl = `${environment.apiUrl}/api/botsettings`;

    constructor(private http: HttpClient, private authService: AuthService) { }

    private getAuthHeaders(): HttpHeaders {
        const token = this.authService.getToken();
        return new HttpHeaders({
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        });
    }

    getSettings(): Observable<BotSettings> {
        return this.http.get<BotSettings>(this.apiUrl, { headers: this.getAuthHeaders() });
    }

    updateComponent(componentName: string, enabled: boolean): Observable<any> {
        return this.http.put(
            `${this.apiUrl}/components/${componentName}`,
            enabled,
            { headers: this.getAuthHeaders() }
        );
    }

    updateTimeout(timeoutName: string, valueSeconds: number): Observable<any> {
        return this.http.put(
            `${this.apiUrl}/timeouts/${timeoutName}`,
            valueSeconds,
            { headers: this.getAuthHeaders() }
        );
    }
}
