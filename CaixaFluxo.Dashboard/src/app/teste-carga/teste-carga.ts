import { Component, ChangeDetectorRef } from '@angular/core'; // ◄ Injetando o ChangeDetectorRef
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

interface ResultadoRequest {
  id: number;
  status: 'SUCESSO' | 'ERRO';
  statusHttp: number;
  tempoMs: number;
  mensagem: string;
}

@Component({
  selector: 'app-teste-carga',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teste-carga.html',
  styleUrls: ['./teste-carga.css']
})
export class TesteCargaComponent {
  apiUrl: string = 'http://localhost:5295/api/lancamentos'; 
  volumeTestes: number = 2; 
  executando: boolean = false;
  tempoTotal: number = 0;
  taxaSucesso: number = 0;
  totalSucessos: number = 0;
  totalErros: number = 0;
  historico: ResultadoRequest[] = [];

  // ◄ Adicionamos o cdr no construtor para forçar a tela a atualizar
  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  iniciarTesteCarga() {
    if (this.volumeTestes <= 0) {
      alert('Por favor, defina um volume maior que zero.');
      return;
    }

    this.executando = true;
    this.historico = [];
    this.totalSucessos = 0;
    this.totalErros = 0;
    this.tempoTotal = 0;
    
    const startTime = performance.now();
    const chamadas: Observable<any>[] = [];

    for (let i = 1; i <= this.volumeTestes; i++) {
      const payloadFalso = {
        valor: Math.floor(Math.random() * 500) + 1,
        tipo: i % 2 === 0 ? 'Credito' : 'Debito',
        descricao: `Teste de Carga #${i}`
      };

      const requestStartTime = performance.now();
      
      const disparo$ = this.http.post(this.apiUrl, payloadFalso).pipe(
        tap({
          next: () => {
            this.totalSucessos++;
            const requestEndTime = performance.now();
            const tempoGasto = Math.round(requestEndTime - requestStartTime);
            
            this.historico.push({
              id: i,
              status: 'SUCESSO',
              statusHttp: 201,
              tempoMs: tempoGasto,
              mensagem: 'Lançamento efetuado com sucesso!'
            });
            this.cdr.detectChanges(); // ◄ Avisa a tela a cada sucesso recebido
          }
        }),
        catchError((err) => {
          this.totalErros++;
          const requestEndTime = performance.now();
          const tempoGasto = Math.round(requestEndTime - requestStartTime);
          
          this.historico.push({
            id: i,
            status: 'ERRO',
            statusHttp: err.status || 500,
            tempoMs: tempoGasto,
            mensagem: 'Falha no processamento ou CORS.'
          });
          this.cdr.detectChanges(); // ◄ Avisa a tela a cada erro recebido
          return of(null); 
        })
      );

      chamadas.push(disparo$);
    }

    // 2. DISPARA TODAS AS CHAMADAS SIMULTANEAMENTE
    forkJoin(chamadas).subscribe({
      next: () => {
        const endTime = performance.now();
        this.tempoTotal = Math.round(endTime - startTime);
        this.taxaSucesso = Math.round((this.totalSucessos / this.volumeTestes) * 100);
        this.executando = false; 
        
        // ◄ FORÇA O ANGULAR A RE-RENDERIZAR A TELA E DESTRAVAR O BOTÃO IMEDIATAMENTE
        this.cdr.detectChanges(); 
        this.cdr.markForCheck();
      },
      error: (globalErr) => {
        console.error('Erro catastrófico no lote:', globalErr);
        this.executando = false;
        this.cdr.detectChanges();
      }
    });
  }
}
