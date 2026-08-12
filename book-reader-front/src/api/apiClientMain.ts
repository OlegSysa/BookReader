import { ENDPOINTS } from "./endpoints";
import type { ApiResponse } from "./abstract/http";

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