#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include <vector>
#include <algorithm>
#include <random>
#include <thread>
#include <mutex>

#pragma comment(lib, "Ws2_32.lib")

class TicTacToeGame {
private:
    std::vector<std::vector<char>> board;
    SOCKET clientSocket;
    std::random_device rd;
    std::mt19937 gen;
    std::mutex mtx;

public:
    TicTacToeGame(SOCKET socket) : board(3, std::vector<char>(3, ' ')), clientSocket(socket), gen(rd()) {}

    void playGame() {
        // Logica principale del gioco
        resetBoard();
        while (true) {
            if (!receiveMove()) break;
            if (checkWin('X')) {
                send(clientSocket, "WIN", 3, 0);
                break;
            }
            if (isBoardFull()) {
                send(clientSocket, "DRAW", 4, 0);
                break;
            }
            makeMove();
            if (checkWin('O')) {
                send(clientSocket, "LOSE", 4, 0);
                break;
            }
            if (isBoardFull()) {
                send(clientSocket, "DRAW", 4, 0);
                break;
            }
        }
        closesocket(clientSocket);
    }

private:
    void resetBoard() {
        // Resetta la board all'inizio di una nuova partita
        std::lock_guard<std::mutex> lock(mtx);
        for (auto& row : board) {
            std::fill(row.begin(), row.end(), ' ');
        }
    }

    bool receiveMove() {
        // Riceve la mossa dal client
        char buffer[1024];
        int bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0);
        if (bytesReceived > 0) {
            std::lock_guard<std::mutex> lock(mtx);
            int row = buffer[0] - '0';
            int col = buffer[2] - '0';
            board[row][col] = 'X';
            return true;
        }
        return false;
    }

    void makeMove() {
        // Esegue la mossa del server
        std::lock_guard<std::mutex> lock(mtx);
        std::pair<int, int> bestMove = findBestMove();
        board[bestMove.first][bestMove.second] = 'O';
        std::string move = std::to_string(bestMove.first) + "," + std::to_string(bestMove.second);
        send(clientSocket, move.c_str(), move.length(), 0);
    }

    // Trova la miglior mossa per il server
    std::pair<int, int> findBestMove() {
        // Controlla se c'è una mossa vincente
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (board[i][j] == ' ') {
                    board[i][j] = 'O';
                    if (checkWin('O')) {
                        board[i][j] = ' ';
                        return { i, j };
                    }
                    board[i][j] = ' ';
                }
            }
        }

        // Blocca una possibile mossa vincente dell'avversario
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (board[i][j] == ' ') {
                    board[i][j] = 'X';
                    if (checkWin('X')) {
                        board[i][j] = ' ';
                        return { i, j };
                    }
                    board[i][j] = ' ';
                }
            }
        }

        // Prova a prendere il centro
        if (board[1][1] == ' ') {
            return { 1, 1 };
        }

        // Prova a prendere un angolo
        std::vector<std::pair<int, int>> corners = { {0, 0}, {0, 2}, {2, 0}, {2, 2} };
        std::shuffle(corners.begin(), corners.end(), gen);
        for (const auto& corner : corners) {
            if (board[corner.first][corner.second] == ' ') {
                return corner;
            }
        }

        // Prende un lato disponibile
        std::vector<std::pair<int, int>> sides = { {0, 1}, {1, 0}, {1, 2}, {2, 1} };
        std::shuffle(sides.begin(), sides.end(), gen);
        for (const auto& side : sides) {
            if (board[side.first][side.second] == ' ') {
                return side;
            }
        }

        // Questo non dovrebbe mai accadere se la scacchiera non è piena
        return { -1, -1 };
    }

    // Controlla se un giocatore ha vinto
    bool checkWin(char player) {
        // Controlla righe, colonne e diagonali
        for (int i = 0; i < 3; i++) {
            if (board[i][0] == player && board[i][1] == player && board[i][2] == player) return true;
            if (board[0][i] == player && board[1][i] == player && board[2][i] == player) return true;
        }
        if (board[0][0] == player && board[1][1] == player && board[2][2] == player) return true;
        if (board[0][2] == player && board[1][1] == player && board[2][0] == player) return true;
        return false;
    }

    // Controlla se la scacchiera è piena
    bool isBoardFull() {
        for (const auto& row : board) {
            for (char cell : row) {
                if (cell == ' ') return false;
            }
        }
        return true;
    }
};

class TicTacToeServer {
private:
    SOCKET listenSocket;
    std::vector<std::thread> gameThreads;

public:
    void start() {
        //Inizializzazione del server
        WSADATA wsaData;
        if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
            std::cerr << "WSAStartup fallito.\n";
            return;
        }

        //Creazione del socket
        listenSocket = socket(AF_INET, SOCK_STREAM, 0);
        if (listenSocket == INVALID_SOCKET) {
            std::cerr << "Errore nella creazione del socket: " << WSAGetLastError() << "\n";
            WSACleanup();
            return;
        }

        //Configurazione dell'indirizzo del server
        sockaddr_in serverAddr;
        serverAddr.sin_family = AF_INET;
        serverAddr.sin_addr.s_addr = INADDR_ANY;
        serverAddr.sin_port = htons(12345);

        //Binding del socket
        if (bind(listenSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
            std::cerr << "Bind fallito: " << WSAGetLastError() << "\n";
            closesocket(listenSocket);
            WSACleanup();
            return;
        }

        //Messa in ascolto del socket
        if (listen(listenSocket, SOMAXCONN) == SOCKET_ERROR) {
            std::cerr << "Listen fallita: " << WSAGetLastError() << "\n";
            closesocket(listenSocket);
            WSACleanup();
            return;
        }

        std::cout << "Server in ascolto sulla porta 12345...\n";

        //Loop principale del server
        while (true) {
            SOCKET clientSocket = accept(listenSocket, NULL, NULL);
            if (clientSocket == INVALID_SOCKET) {
                std::cerr << "Accept fallita: " << WSAGetLastError() << "\n";
                continue;
            }

            std::cout << "Nuovo client connesso. Avvio una nuova partita...\n";
            gameThreads.push_back(std::thread([this, clientSocket]() {
                TicTacToeGame game(clientSocket);
                game.playGame();
                }));
        }
    }

    ~TicTacToeServer() {
        //Pulizia e chiusura del server
        for (auto& thread : gameThreads) {
            if (thread.joinable()) {
                thread.join();
            }
        }
        closesocket(listenSocket);
        WSACleanup();
    }
};

int main() {
    TicTacToeServer server;
    server.start();
    return 0;
}