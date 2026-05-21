import { ApplicationConfig, Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Catalog } from './catalog/catalog';
import { provideHttpClient, withFetch } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
    providers: [provideHttpClient(withFetch())]
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Catalog],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {

  
  protected readonly title = signal('Naruto-app');
}
