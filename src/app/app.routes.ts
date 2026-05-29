import { Routes } from '@angular/router';
import { Catalog } from './catalog/catalog';
import { Home } from './home/home';
import { ShinobiFormComponent } from './shinobi-form/shinobi-form';
import { JutsuForm } from './jutsu-form/jutsu-form';

export const routes: Routes = [
    
    
    {
        title: "Home",
        path: "",
        component: Home
    },
    
    {
        title: "Catalog",
        path: "catalog",
        component: Catalog
    },

    {
        title: "Shinobi Form",
        path: "shinobi-form",
        component: ShinobiFormComponent
    },
    {
        title: "Jutsu Form",
        path: "jutsu-form",
        component: JutsuForm
    }
    
];
