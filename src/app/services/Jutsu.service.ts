import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ShinobiModel } from "../models/shinobi.model";    
import { JutsuModel } from "../models/Jutsus.model";
import { Observable } from "rxjs";
import { Global } from "../global";

@Injectable({
    providedIn: 'root'
})
export class jutsuService {

    private http = inject(HttpClient)
    private apiUrl = Global.baseURL + "/api/jutsus";

    getJutsus() {
        return this.http.get<JutsuModel[]>(`${this.apiUrl}`);
    }
    getJutsuById(id: number) {
        return this.http.get<JutsuModel>(`${this.apiUrl}/${id}`);
    }
    createJutsu(jutsu: JutsuModel): Observable<JutsuModel> {
        return this.http.post<JutsuModel>(this.apiUrl, jutsu);
    }
    updateJutsu(jutsu: JutsuModel): Observable<JutsuModel> {
        return this.http.put<JutsuModel>(`${this.apiUrl}/${jutsu.id}`, jutsu);
    }
    deleteJutsu(id: number): Observable<JutsuModel> {
        return this.http.delete<JutsuModel>(`${this.apiUrl}/${id}`);
    }


}