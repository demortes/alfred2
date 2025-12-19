import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CommonModule, DatePipe } from '@angular/common';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-mock-weather',
  templateUrl: './mock-weather.component.html',
  styleUrls: ['./mock-weather.component.less'],
  standalone: true,
  imports: [CommonModule, DatePipe]
})
export class MockWeatherComponent implements OnInit {
  public weather: WeatherForecast[] = [];

  /**
   *
   */
  constructor(private httpClient: HttpClient) {


  }

  ngOnInit(): void {
    this.httpClient.get<WeatherForecast[]>(environment.apiUrl + "/WeatherForecast").subscribe({
      next: (response: any) => {
        this.weather = response;
      },
      error: (error: any) => {
        console.error('Error fetching weather data:', error);
      }
    });
  }

}
