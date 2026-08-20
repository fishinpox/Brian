import { useState } from 'react';
import { Link } from 'react-router-dom';
import { decodeJwt, getToken } from '../lib/auth';
import { useMySite, useProvisionSite, usePublishSite, useUnpublishSite } from '../lib/queries/vtuberHubQueries';

function slugify(input: string): string {
  return input
    .toLowerCase()
    .replace(/[^a-z0-9-]+/g, '-')
    .replace(/^-+|-+$/g, '')
    .slice(0, 30);
}

export function VTuberHubPage() {
  const token = getToken();
  const claims = token ? decodeJwt(token) : null;
  const suggestedSlug = typeof claims?.unique_name === 'string' ? slugify(claims.unique_name) : '';

  const mySite = useMySite();
  const provision = useProvisionSite();
  const publish = usePublishSite();
  const unpublish = useUnpublishSite();

  const [slugInput, setSlugInput] = useState(suggestedSlug);
  const [error, setError] = useState<string | null>(null);

  function handleProvision() {
    setError(null);
    const slug = slugify(slugInput);
    if (slug.length < 3) {
      setError('Site address must be at least 3 characters.');
      return;
    }
    provision.mutate(slug, {
      onError: () => setError('Could not create your site. That address may already be taken.'),
    });
  }

  const site = mySite.data;

  return (
    <div className="mx-auto max-w-3xl px-6 py-10">
      <header className="mb-8">
        <h1 className="text-xl font-semibold text-gray-800">VTuberHub</h1>
        <p className="mt-0.5 text-xs text-gray-500">Your branded public creator website</p>
      </header>

      <div className="rounded-lg bg-white p-6 shadow-sm">
        <Link to="/" className="mb-4 inline-block text-xs text-blue-500 hover:underline">
          &larr; Back to Calendar
        </Link>

        {mySite.isLoading && <p className="py-5 text-center text-sm text-gray-400">Loading...</p>}

        {!mySite.isLoading && !site && (
          <div>
            <p className="mb-4 text-sm text-gray-600">
              You don't have a site yet. Choose an address to get started - you can change what's on it later.
            </p>
            <div className="mb-3 flex items-center gap-2">
              <span className="text-sm text-gray-500">vtuberhub.com/</span>
              <input
                type="text"
                value={slugInput}
                onChange={(e) => setSlugInput(e.target.value)}
                placeholder="your-name"
                className="flex-1 rounded-md border border-gray-300 px-3 py-2 text-sm"
              />
            </div>
            {error && <div className="mb-3 text-xs text-red-600">{error}</div>}
            <button
              type="button"
              onClick={handleProvision}
              disabled={provision.isPending}
              className="rounded-md bg-blue-500 px-5 py-2.5 text-sm text-white transition hover:bg-blue-600 disabled:opacity-50"
            >
              Create my site
            </button>
          </div>
        )}

        {site && (
          <div>
            <dl className="mb-4 space-y-2 text-sm">
              <div className="flex justify-between">
                <dt className="text-gray-500">Address</dt>
                <dd className="text-gray-800">vtuberhub.com/{site.slug}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Status</dt>
                <dd className="text-gray-800">{site.publishState}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Your access</dt>
                <dd className="text-gray-800">{site.myAccessLevel}</dd>
              </div>
            </dl>

            {site.myAccessLevel === 'Admin' && (
              <button
                type="button"
                onClick={() =>
                  site.publishState === 'Published' ? unpublish.mutate(site.id) : publish.mutate(site.id)
                }
                disabled={publish.isPending || unpublish.isPending}
                className="rounded-md bg-blue-500 px-5 py-2.5 text-sm text-white transition hover:bg-blue-600 disabled:opacity-50"
              >
                {site.publishState === 'Published' ? 'Unpublish' : 'Publish'}
              </button>
            )}

            <p className="mt-4 text-xs text-gray-400">
              Page content and theming aren't built yet - this is just your site's address and publish state for
              now.
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
