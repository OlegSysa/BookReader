export interface ApiResponse<T> {
    data: T;
    code: number;
    success: boolean;
    errorMessage: string;
}