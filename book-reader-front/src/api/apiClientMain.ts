import { ENDPOINTS } from "./endpoints";
import type { ApiResponse } from "./abstract/http";

export async function Register(
    email: string,
    password: string
): Promise<ApiResponse<string>> {
    const response = await fetch(
        ENDPOINTS.register(email, password)
    );

    if (!response.ok) {
        throw new Error("Failed to register user");
    }

    return await response.json();
}