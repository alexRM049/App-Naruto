import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ShinobiModel } from "../catalog/shinobi.model";    

@Injectable({
    providedIn: 'root'
})
export class ShinobiService {
    constructor(private http: HttpClient) { }
    
    getShinobi() {
        return this.http.get<ShinobiModel[]>('/api/shinobis');
    }


}