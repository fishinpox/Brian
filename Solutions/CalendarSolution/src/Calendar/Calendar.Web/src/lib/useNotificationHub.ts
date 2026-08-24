import { useEffect } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { createNotificationConnection } from './signalr';
import { BACKGROUND_KEY } from './queries/backgroundQueries';

export function useNotificationHub() {
  const queryClient = useQueryClient();

  useEffect(() => {
    const connection = createNotificationConnection();
    connection.on('calendar-background-ready', () => {
      queryClient.invalidateQueries({ queryKey: BACKGROUND_KEY });
    });

    let stopped = false;
    connection.start().catch((err) => {
      if (!stopped) console.error('SignalR connection failed:', err);
    });

    return () => {
      stopped = true;
      connection.stop();
    };
  }, []);
}
