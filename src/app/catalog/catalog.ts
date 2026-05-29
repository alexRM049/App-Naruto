import { afterNextRender, Component, computed, inject, OnInit, signal } from '@angular/core';
import { ShinobiService } from '../services/Shinobi.service';
import { ShinobiModel } from '../models/shinobi.model';
import { JutsuModel } from '../models/Jutsus.model';
import { villagesService } from '../services/Villages.service';
import { VillageModel } from '../models/village.model';
import { RouterLink } from "@angular/router";



@Component({
  selector: 'app-catalog',
  imports: [RouterLink],
  templateUrl: './catalog.html',
  styleUrl: './catalog.css',
})
export class Catalog {
  private shinobiService = inject(ShinobiService);

    // Form State Temporary Inputs
  selectedJutsus = signal<string[]>([]);
  selectedShinobis = signal<string[]>([]);
  villagesService = inject(villagesService)
  shinobi = signal<ShinobiModel[]>([]);
  villageFilter = signal<number>(0);
  rankFilter = signal<string>('');
  searchFilter = signal<string>('');
  villageNames = signal<any>('');
  filteredShinobis = signal<ShinobiModel[]>([]);
  //filteredVillageIds = signal<VillageModel | null>(null);

  
  
  // Navigation State Routing Signal
  currentView = signal<'home' | 'catalog' | 'add-shinobi' | 'add-jutsu' | 'add-village'>('home');


  // State Routing Controller
  setView(view: 'home' | 'catalog' | 'add-shinobi' | 'add-jutsu' | 'add-village') {
    this.currentView.set(view);
    // Reset temporary states on routing
    if (view === 'add-shinobi') {
      this.selectedJutsus.set([]);
    } else if (view === 'add-jutsu') {
      this.selectedShinobis.set([]);
    }
  }

constructor() {
  afterNextRender(() => {

    
    console.log("Running safely in the browser!")
    this.filterShinobis();
    this.loadDropDownData();
    this.villagesService.getVillages().subscribe({
      next: (data: any) => {
      },
      error: (err: Error) => {

      }
    })
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



/*filterShinobis(){
  this.filteredShinobis.set(
    this.shinobi().filter(ninja => {
      const currentVillage = this.villageNames()?.id;
      const matchSearch = ninja.name.toLowerCase().includes(this.searchFilter().toLowerCase());
      const matchVillage = !this.villageFilter() || ninja.villageId === currentVillage;
      const matchRank = !this.rankFilter() || ninja.rank === this.rankFilter();
      return matchSearch && matchVillage && matchRank;
    })
  )
}*/

shinobis = this.shinobi

filterShinobis = computed(() => {
  const search = this.searchFilter().toLowerCase().trim();
  const rank = this.rankFilter();
  
  // 1. Get the raw value from the dropdown signal
  const rawVillageFilter = this.villageFilter(); 
  
  // 2. Cast string to number if a value exists, otherwise keep it null/0
  const targetVillageId = rawVillageFilter ? Number(rawVillageFilter) : 0;

  return this.shinobis().filter(ninja => {
    const matchSearch = !search || ninja.name.toLowerCase().includes(search);
    const matchRank = !rank || ninja.rank === rank;
    
    // 3. Compare ninja.villageId directly with your target number value
    const matchVillage = !targetVillageId || ninja.villageId === targetVillageId;

    return matchSearch && matchRank && matchVillage;
  });
});

 private loadDropDownData(){
  this.villagesService.getVillageName().subscribe({
    next: (data: any) => {
      this.villageNames.set(data);
      console.log("---------------data------------:", data)
    },
    error: (err: Error) => {
      console.log("---------------error------------:", err)
    }
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
