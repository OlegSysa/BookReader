import { ENDPOINTS } from "./endpoints";
import type { ApiResponse } from "./models/http";

export async function Register(
    email: string,
    password: string
): Promise<ApiResponse<string>> {
    const response = await fetch(
        ENDPOINTS.register(),
        {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                email,
                password
            })
        }
    );

    return await response.json();
}

export async function Login(
    email: string,
    password: string
): Promise<ApiResponse<string>> {
    const response = await fetch(
        ENDPOINTS.login(),
        {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                email,
                password
            })
        }
    );

    return await response.json();
}

export async function Logout(): Promise<void> {
    const response = await fetch(
        ENDPOINTS.logout(),
        {
            method: "POST",
            credentials: "include"
        }
    );

    if (!response.ok) {
        throw new Error("Failed to logout");
    }
}