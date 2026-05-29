import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ShinobiService } from '../services/Shinobi.service';
import { ShinobiModel } from '../models/shinobi.model';
import { villagesService } from '../services/Villages.service'
import { VillageModel } from '../models/village.model';
import { Router, RouterLink, ActivatedRoute, RouterOutlet } from '@angular/router';


@Component({
  selector: 'app-shinobi-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './shinobi-form.html',
  styleUrl: './shinobi-form.css'
})
export class ShinobiFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private shinobiService = inject(ShinobiService);
  private villageService = inject(villagesService);
  private router = inject(Router);

  // Writable signal to feed data to our village dropdown selector
  villages = signal<VillageModel[]>([]);
  
  // Track static rank list locally
  readonly ranks = ['Genin', 'Chunin', 'Jonin'];
  
  shinobiForm!: FormGroup;

  ngOnInit() {
    this.initForm();
    this.loadVillages();
  }

  private initForm() {
    this.shinobiForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      rank: ['Genin', Validators.required],
      photoUrl: [''],
      villageId: ['', Validators.required] // Must match a valid primary key from the DB
    });
  }

  private loadVillages() {
    this.villageService.getVillages().subscribe({
      next: (data) => this.villages.set(data),
      error: (err) => console.error('Failed to load villages for form:', err)
    });
  }

  onSubmit() {
    if (this.shinobiForm.invalid) return;

    // Send the raw form payload straight down to our service layer
    this.shinobiService.createShinobi(this.shinobiForm.value).subscribe({
      next: (newShinobi: ShinobiModel) => {
        console.log('Successfully created Shinobi:', newShinobi);
        // Reset the form layout to clean defaults
        this.shinobiForm.reset({ rank: 'Genin', villageId: '' });
      },
      error: (err: any) => console.error('Backend submission rejected:', err)
    });
  }
}