// shinobi.model.ts
export interface ShinobiModel {
    id: number;
    name: string;
    rank: string;
    photoUrl?: string;
    villageId: number;
    village?: { name: string; kageTitle: string };
    jutsus: Array<{ name: string; element: string }>;
}