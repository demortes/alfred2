import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-mock-weather',
  templateUrl: './mock-weather.component.html',
  styleUrls: ['./mock-weather.component.less'],
  standalone: true,
  imports: [JsonPipe]
})
export class MockWeatherComponent implements OnInit {
  public weather = {};

  /**
   *
   */
  constructor(private httpClient: HttpClient) {


  }

  ngOnInit(): void {
    interface WeatherResponse {
      // Add properties based on the expected response structure
      // For example:
      temperature: number;
      condition: string;
      humidity: number;
      [key: string]: any;
    }

    this.httpClient.get<WeatherResponse>(environment.apiUrl + "/weather").subscribe((response: WeatherResponse) => {
      this.weather = response;
    })
  }

}
