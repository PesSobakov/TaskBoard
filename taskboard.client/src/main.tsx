import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { BrowserRouter, Route } from "react-router";
import Home from './app/Home.tsx';
import Login from './app/Login.tsx';
import Board from './app/Board.tsx';
import { Routes } from 'react-router';
import Account from './app/Account.tsx';
import Register from './app/Register.tsx';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <BrowserRouter>
            <Routes>
                <Route element={<App key={`app-${0}`} />}>
                    <Route index element={<Home key={`home-${0}`} />} />
                    <Route path="login" element={<Login key={`login-${0}`} />} />
                    <Route path="register" element={<Register key={`register-${0}`} />} />
                    <Route path="account" element={<Account key={`account-${0}`} />} />
                    <Route path="board/:id" element={<Board key={`board-${0}`} />} />
                    <Route path="board/:id/card/:cardId" element={<Board key={`board-${0}`} />} />
                </Route>
            </Routes>
        </BrowserRouter>
    </StrictMode>,
)
