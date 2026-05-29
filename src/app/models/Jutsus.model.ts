export interface JutsuModel {
  id: string;
  name: string;
  element: string;
  shinobi: string[]; // references Shinobi.id
}