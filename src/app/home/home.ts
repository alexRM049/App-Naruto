import { Component,signal, computed, ChangeDetectionStrategy } from '@angular/core';
import {CommonModule} from '@angular/common';
import { RouterLink } from "@angular/router";

// Updated interfaces to match your API schema
interface Village {
  id: string;
  name: string;
  kageTitle: string;
}

interface Jutsu {
  id: string;
  name: string;
  element: string;
  shinobi: string[]; // references Shinobi.id
}

interface Shinobi {
  id: string;
  name: string;
  photoUrl: string;
  villageId: string;
  village: string; // raw village name string
  jutsus: string[]; // references Jutsu.id
  rank: string;
}

interface Toast {
  id: number;
  title: string;
  message: string;
}
@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
   // Navigation State Routing Signal
  currentView = signal<'home' | 'catalog' | 'add-shinobi' | 'add-jutsu' | 'add-village'>('home');

  // Relational Database Signals (API-structured)
  villages = signal<Village[]>([
    { id: 'v-1', name: 'Hidden Leaf Village', kageTitle: 'Hokage' },
    { id: 'v-2', name: 'Hidden Sand Village', kageTitle: 'Kazekage' },
    { id: 'v-3', name: 'Hidden Rain Village', kageTitle: 'Leader' }
  ]);

  jutsus = signal<Jutsu[]>([
    { id: 'j-1', name: 'Rasengan', element: 'Wind', shinobi: ['s-1'] },
    { id: 'j-2', name: 'Lightning Blade (Chidori)', element: 'Lightning', shinobi: ['s-2'] },
    { id: 'j-3', name: 'Amaterasu', element: 'Fire', shinobi: ['s-2'] }
  ]);

  shinobis = signal<Shinobi[]>([
    { 
      id: 's-1',
      name: 'Naruto Uzumaki', 
      photoUrl: '', // blank demonstrates generic initials fallback styling
      villageId: 'v-1',
      village: 'Hidden Leaf Village', 
      jutsus: ['j-1'], 
      rank: 'Kage'
    },
    { 
      id: 's-2',
      name: 'Sasuke Uchiha', 
      photoUrl: '', 
      villageId: 'v-1',
      village: 'Hidden Leaf Village', 
      jutsus: ['j-2', 'j-3'], 
      rank: 'Rogue Ninja'
    }
  ]);

  // Form State Temporary Inputs
  selectedJutsus = signal<string[]>([]);
  selectedShinobis = signal<string[]>([]);

  // Filter Signals
  searchFilter = signal<string>('');
  villageFilter = signal<string>('');
  rankFilter = signal<string>('');

  // Toast System Signal
  toasts = signal<Toast[]>([]);

  // Filtered computed roster data
  filteredShinobis = computed(() => {
    const search = this.searchFilter().toLowerCase();
    const villageId = this.villageFilter();
    const rank = this.rankFilter();

    return this.shinobis().filter(ninja => {
      const matchSearch = ninja.name.toLowerCase().includes(search);
      const matchVillage = !villageId || ninja.villageId === villageId;
      const matchRank = !rank || ninja.rank === rank;
      return matchSearch && matchVillage && matchRank;
    });
  });

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

  // Trigger non-blocking notifications
  triggerToast(title: string, message: string) {
    const id = Date.now();
    this.toasts.update(current => [...current, { id, title, message }]);
    setTimeout(() => {
      this.toasts.update(current => current.filter(t => t.id !== id));
    }, 4000);
  }

  // Generate initials for generic profile display when no photo is provided
  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
  }

  // Resolve Jutsu ID to name
  getJutsuName(id: string): string {
    const match = this.jutsus().find(j => j.id === id);
    return match ? `${match.name} [${match.element}]` : 'Unknown Scroll';
  }

  // Checkbox state tracker for Jutsus
  toggleJutsuSelection(id: string) {
    this.selectedJutsus.update(current => {
      if (current.includes(id)) {
        return current.filter(jId => jId !== id);
      } else {
        return [...current, id];
      }
    });
  }

  // Checkbox state tracker for Shinobis inside Jutsu registration
  toggleShinobiSelection(id: string) {
    this.selectedShinobis.update(current => {
      if (current.includes(id)) {
        return current.filter(sId => sId !== id);
      } else {
        return [...current, id];
      }
    });
  }

  // Submission: Shinobi
  createShinobi(event: Event) {
    event.preventDefault();
    const target = event.target as HTMLFormElement;
    const name = (target.querySelector('#shinobiName') as HTMLInputElement).value;
    const photoUrl = (target.querySelector('#shinobiPhoto') as HTMLInputElement).value;
    const villageId = (target.querySelector('#shinobiVillageId') as HTMLSelectElement).value;
    const rank = (target.querySelector('#shinobiRank') as HTMLSelectElement).value;

    const matchedVillageObj = this.villages().find(v => v.id === villageId);
    const villageName = matchedVillageObj ? matchedVillageObj.name : 'Unknown Territory';
    const newShinobiId = `s-${Date.now()}`;

    const newShinobi: Shinobi = {
      id: newShinobiId,
      name,
      photoUrl,
      villageId,
      village: villageName,
      jutsus: this.selectedJutsus(),
      rank
    };

    // Update state
    this.shinobis.update(current => [newShinobi, ...current]);

    // Track bidirectional updates safely in client-memory (simulate API behavior)
    const selectedJutsusList = this.selectedJutsus();
    this.jutsus.update(currentJutsus => {
      return currentJutsus.map(j => {
        if (selectedJutsusList.includes(j.id) && !j.shinobi.includes(newShinobiId)) {
          return { ...j, shinobi: [...j.shinobi, newShinobiId] };
        }
        return j;
      });
    });

    this.triggerToast("Shinobi Registered", `${name} is now synchronized into the archive.`);
    this.setView('catalog');
  }

  // Submission: Jutsu Scroll
  createJutsu(event: Event) {
    event.preventDefault();
    const target = event.target as HTMLFormElement;
    const name = (target.querySelector('#jutsuName') as HTMLInputElement).value;
    const element = (target.querySelector('#jutsuElement') as HTMLSelectElement).value;
    const newJutsuId = `j-${Date.now()}`;

    const newJutsu: Jutsu = {
      id: newJutsuId,
      name,
      element,
      shinobi: this.selectedShinobis()
    };

    this.jutsus.update(current => [...current, newJutsu]);

    // Sync bidirectional relationship back to Shinobis
    const selectedShinobisList = this.selectedShinobis();
    this.shinobis.update(currentShinobis => {
      return currentShinobis.map(s => {
        if (selectedShinobisList.includes(s.id) && !s.jutsus.includes(newJutsuId)) {
          return { ...s, jutsus: [...s.jutsus, newJutsuId] };
        }
        return s;
      });
    });

    this.triggerToast("Scroll Recorded", `The technique "${name}" is now stored.`);
    this.setView('add-shinobi');
  }

  // Submission: Village Region
  createVillage(event: Event) {
    event.preventDefault();
    const target = event.target as HTMLFormElement;
    const name = (target.querySelector('#villageName') as HTMLInputElement).value;
    const kageTitle = (target.querySelector('#villageKageTitle') as HTMLInputElement).value;

    const newVillage: Village = {
      id: `v-${Date.now()}`,
      name,
      kageTitle
    };

    this.villages.update(current => [...current, newVillage]);
    this.triggerToast("Map Updated", `Geopolitical boundaries updated. ${name} is now active.`);
    this.setView('add-shinobi');
  }
}
