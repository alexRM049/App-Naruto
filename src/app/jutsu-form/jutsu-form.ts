import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ShinobiService } from '../services/Shinobi.service';
import { jutsuService } from '../services/Jutsu.service';
import { JutsuModel } from '../models/Jutsus.model';
import { ShinobiModel } from '../models/shinobi.model';

@Component({
  selector: 'app-jutsu-form',
  imports: [ReactiveFormsModule],
  templateUrl: './jutsu-form.html',
  styleUrl: './jutsu-form.css',
})
export class JutsuForm {

  private fb = inject(FormBuilder)
  private router = inject(Router)
  private service = inject(ShinobiService)
  private jutsuService = inject(jutsuService)
  shinobis = signal<ShinobiModel[]>([]);
 
  
  readonly elements = ['Fire', 'Water', 'Earth', 'Wind', 'Lightning', 'Ice', 'Lava', 'Wood', 'Steel', 'Yin', 'Yang', 'Yin-Yang'];

  jutsuForm!: FormGroup;

  
  
  ngOnInit(): void {
    this.iniForm();
    this.loadShinobis();
    
  }

  private iniForm() {
    this.jutsuForm = this.fb.group({
      name: ['', Validators.required],
      element: ['', Validators.required],
      shinobiId: ['', Validators.required],

      
    });
  }

  private loadShinobis() {
    this.service.getShinobi().subscribe({
      next: (data: any) => this.shinobis.set(data),
      error: (err: any) => console.error('Failed to load shinobi for form:', err)
    });
  }

  onSubmit() {
    if(this.jutsuForm.invalid) return alert('Please fill in all the fields');

    this.jutsuService.createJutsu(this.jutsuForm.value).subscribe({
      next: (newJutsu: JutsuModel) => {
        console.log(`New Jutsu Created: ${JSON.stringify(newJutsu)}`)

        this.jutsuForm.reset({
          name: '',
          element: '',
          shinobi: ''
        })
      },
      
      error: (err: any) => console.error('Failed to create jutsu:', err)
    })
      

    }
      
      
    

}
