import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from 'express';
import { villagesService } from '../services/Villages.service';

@Component({
  selector: 'app-village-form',
  imports: [],
  templateUrl: './village-form.html',
  styleUrl: './village-form.css',
})
export class VillageForm {

  private fb = inject(FormBuilder)
  private villageService = inject(villagesService)
  private router = inject(Router)


  villageForm!: FormGroup;

  ngOnInit(): void {
    this.initForm()
  }

  private initForm() {
    this.villageForm = this.fb.group({
      name: ['', Validators.required],
      kageTitle: ['', Validators.required],

    });
  }

  
}
