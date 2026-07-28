import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiFetch, apiFetchJson, withJsonBody } from '../apiClient';
import type { PlatformConfig } from '../../config/platforms';

export interface Channel {
  channelId: string;
  name: string;
  englishName?: string;
  photoUrl?: string;
  isFollowed?: boolean;
}

interface ConnectionStatus {
  hasCredential: boolean;
}

export function useConnectionStatus(config: PlatformConfig) {
  return useQuery({
    queryKey: ['connection-status', config.key],
    queryFn: () => apiFetchJson<ConnectionStatus>(config.followedBaseUrl, config.statusEndpoint),
  });
}

export function useFollowedChannels(config: PlatformConfig, enabled: boolean) {
  return useQuery({
    queryKey: ['followed-channels', config.key],
    queryFn: () => apiFetchJson<Channel[]>(config.followedBaseUrl, config.followedEndpoint),
    enabled,
  });
}

export function useSearchChannels(config: PlatformConfig, term: string) {
  return useQuery({
    queryKey: ['search-channels', config.key, term],
    queryFn: () =>
      apiFetchJson<Channel[]>(config.searchBaseUrl, `${config.searchEndpoint}?q=${encodeURIComponent(term)}`),
    enabled: term.trim().length >= 2,
  });
}

export function useBrowseChannels(config: PlatformConfig) {
  return useQuery({
    queryKey: ['browse-channels', config.key],
    queryFn: () => apiFetchJson<Channel[]>(config.searchBaseUrl, config.searchEndpoint),
  });
}

export function useSaveCredential(config: PlatformConfig) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (apiKey: string) =>
      apiFetch(config.followedBaseUrl, config.credentialEndpoint, withJsonBody({ apiKey })),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['connection-status', config.key] });
    },
  });
}

export function useSaveFollowed(config: PlatformConfig) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (channels: Channel[]) =>
      apiFetch(config.followedBaseUrl, config.followedEndpoint, withJsonBody({ channels })),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['followed-channels', config.key] });
    },
  });
}
