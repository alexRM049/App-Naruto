import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Global } from "../global";
import { ShinobiModel } from "../catalog/shinobi.model";
import { Observable } from "rxjs";


@Injectable({
    providedIn: "root"
})
export class ShinobiService {
    private baseUrl: string = Global.baseURL;
    private http = inject(HttpClient);
    
    getShinobi() {
        const response =  this.http.get(`${this.baseUrl}/api/Shinobis`)
        return response
    }

    createShinobi(shinobi: Partial<ShinobiModel>): Observable<ShinobiModel> {
        return this.http.post<ShinobiModel>(`${this.baseUrl}/api/shinobis`, shinobi);
    }

    updateShinobi(id: number, shinobi: Partial<ShinobiModel>): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/api/shinobis/${id}`, shinobi);
    }

    deleteShinobi(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/api/shinobis/${id}`);
    }
    
    constructor() { }
}