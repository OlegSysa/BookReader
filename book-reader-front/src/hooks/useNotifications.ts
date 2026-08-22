import { useEffect, useState } from "react";
import { ENDPOINTS } from "../api/endpoints";

export function useNotifications() {
    const [bookStatuses, setBookStatuses] = useState<Record<number, number>>({});
    useEffect(() => {
        const eventSource = new EventSource(
            ENDPOINTS.connectNotifications(),
            {
                withCredentials: true
            }
        );
        eventSource.onmessage = (event) => {
            const notification = JSON.parse(event.data);
            setBookStatuses(prev => ({
                ...prev,
                [notification.BookId]: notification.Status
            }));
        };
        eventSource.onopen = () => {
            console.log("SSE connected");
        };

        eventSource.onerror = (error) => {
            console.error("SSE error:", error);
        };

        return () => {
            eventSource.close();
        };
    }, []);
    return bookStatuses;
}