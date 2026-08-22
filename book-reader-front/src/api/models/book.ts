export interface BookModel {
    id: number;
    originalFileName: string;
    fileSize: number;
    status: number;
    createdAtUtc: string;
    chaptersCount: number;
}