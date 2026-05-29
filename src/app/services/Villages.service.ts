import { Injectable, inject, signal } from '@angular/core'
import { HttpClient} from '@angular/common/http'
import { Global } from "../global"
import { VillageModel } from '../models/village.model';
import { Observable } from 'rxjs';
import fil from '@angular/common/locales/fil';



@Injectable({
    providedIn: 'root'
})
export class villagesService {
    private http = inject(HttpClient);
    private apiUrl = Global.baseURL + '/api/villages';
    villageName = signal<any>('')
    
    getVillages() {
        return this.http.get<VillageModel[]>(this.apiUrl);
    }

    getVillageById(id: number) {
        return this.http.get<VillageModel>(`${this.apiUrl}/${id}`);
    }

    createVillage(village: VillageModel): Observable<VillageModel> {
        return this.http.post<VillageModel>(this.apiUrl, village);
    }

    updateVillage(village: VillageModel): Observable<VillageModel> {
        return this.http.put<VillageModel>(`${this.apiUrl}/${village.id}`, village);
    }

    deleteVillage(id: number): Observable<VillageModel> {
        return this.http.delete<VillageModel>(`${this.apiUrl}/${id}`);
    }

   getVillageName(): Observable<VillageModel[]> {
        return this.http.get<VillageModel[]>(`${this.apiUrl}/name`);
   }   


}
