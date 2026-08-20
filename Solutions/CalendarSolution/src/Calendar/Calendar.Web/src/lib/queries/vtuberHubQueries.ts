import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiFetchJson, withJsonBody } from '../apiClient';
import { CREATOR_API } from '../../config/platforms';

export type SitePublishState = 'Draft' | 'Published';
export type SiteAccessLevel = 'User' | 'Admin';

export interface Site {
  id: string;
  ownerProfileId: string;
  slug: string;
  publishState: SitePublishState;
  publishedAt: string | null;
  createdAt: string;
  myAccessLevel: SiteAccessLevel;
}

const MY_SITE_KEY = ['vtuberhub', 'my-site'];

/** Returns null (not an error) when the caller has no site yet - use this to drive the "create" view. */
export function useMySite() {
  return useQuery({
    queryKey: MY_SITE_KEY,
    queryFn: async () => {
      try {
        return await apiFetchJson<Site>(CREATOR_API, '/api/creators/sites/mine');
      } catch {
        return null;
      }
    },
  });
}

export function useProvisionSite() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (requestedSlug: string) =>
      apiFetchJson<Site>(CREATOR_API, '/api/creators/sites/provision', withJsonBody({ requestedSlug })),
    onSuccess: (site) => queryClient.setQueryData(MY_SITE_KEY, site),
  });
}

export function usePublishSite() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (siteId: string) =>
      apiFetchJson<Site>(CREATOR_API, `/api/creators/sites/${siteId}/publish`, { method: 'POST' }),
    onSuccess: (site) => queryClient.setQueryData(MY_SITE_KEY, site),
  });
}

export function useUnpublishSite() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (siteId: string) =>
      apiFetchJson<Site>(CREATOR_API, `/api/creators/sites/${siteId}/unpublish`, { method: 'POST' }),
    onSuccess: (site) => queryClient.setQueryData(MY_SITE_KEY, site),
  });
}
