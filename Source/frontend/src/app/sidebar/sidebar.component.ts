import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
    selector: 'app-sidebar',
    imports: [CommonModule, RouterModule],
    templateUrl: './sidebar.component.html',
    styleUrl: './sidebar.component.less'
})
export class SidebarComponent {
    private router = inject(Router);
    private authService = inject(AuthService);

    isActive(route: string): boolean {
        return this.router.url === route || this.router.url.startsWith(route + '/');
    }

    onLogout(): void {
        this.authService.logout();
    }
}
