import { afterNextRender, Component, inject, OnInit, signal } from '@angular/core';
import { ShinobiService } from '../services/shinobi.service';
import { ShinobiModel } from './shinobi.model';



@Component({
  selector: 'app-catalog',
  imports: [],
  templateUrl: './catalog.html',
  styleUrl: './catalog.css',
})
export class Catalog {
  private shinobiService = inject(ShinobiService);
  shinobi = signal<ShinobiModel[]>([]);

 

constructor() {
  afterNextRender(() => {
    console.log("Running safely in the browser!")

    this.shinobiService.getShinobi().subscribe({
      next: (data: any) => {
        console.log("---------------data-------------:", data);
        this.shinobi.set(data);
      },
      error: (err: Error) => {
        console.log("---------------error------------:", err)
      }
    })
  })
}

  getElementIcon(element: string): string {
    const el = (element || '').toLowerCase().trim();
    switch (el) {
      case 'fire':
      case 'fuego':
        return 'bi-fire';
      case 'water':
      case 'agua':
        return 'bi-droplet-fill';
      case 'earth':
      case 'tierra':
        return 'bi-globe-americas';
      case 'wind':
      case 'viento':
        return 'bi-wind';
      case 'lightning':
      case 'rayo':
        return 'bi-lightning-charge-fill';
      default:
        return 'bi-star-fill';
    }
  }
}
