import { useEffect } from 'react';
import { createNotificationConnection } from './signalr';

/**
 * Connects to the Notifications hub for the lifetime of the app and logs
 * calendar-background-ready pushes. The background-upload UI itself is a
 * separate follow-up (see Documentation/Calendar/FrontEndTechnologyChoices.md) -
 * this hook only proves the SignalR connection, auth, and CORS path work.
 */
export function useNotificationHub() {
  useEffect(() => {
    const connection = createNotificationConnection();
    connection.on('calendar-background-ready', () => {
      console.log('calendar-background-ready notification received');
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
