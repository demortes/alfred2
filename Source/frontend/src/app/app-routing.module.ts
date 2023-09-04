import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MockWeatherComponent } from './mock-weather/mock-weather.component';
import { MsalGuard } from '@azure/msal-angular';

const routes: Routes = [
  {
    path: 'weather',
    component: MockWeatherComponent,
    canActivate: [MsalGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
