import { Component, provideZoneChangeDetection } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { provideHttpClient } from '@angular/common/http'; 
import { TesteCargaComponent } from './teste-carga/teste-carga'; 

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, TesteCargaComponent],
  template: `<app-teste-carga></app-teste-carga>` 
})
export class App {
  // Classe principal limpa do novo formato minimalista
}

// Inicialização da aplicação injetando o suporte ao HttpClient
bootstrapApplication(App, {
  providers: [
    provideHttpClient()
  ]
}).catch((err) => console.error(err));
