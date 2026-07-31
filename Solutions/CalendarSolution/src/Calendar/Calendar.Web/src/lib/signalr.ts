import * as signalR from '@microsoft/signalr';
import { getValidAccessToken } from './auth';
import { NOTIFICATIONS_API } from '../config/platforms';

export function createNotificationConnection(): signalR.HubConnection {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${NOTIFICATIONS_API}/hubs/notifications`, {
      accessTokenFactory: async () => (await getValidAccessToken()) ?? '',
    })
    .withAutomaticReconnect()
    .build();
}
