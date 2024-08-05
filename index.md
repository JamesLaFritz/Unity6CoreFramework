---
# Feel free to add content and custom Front Matter to this file.
# To modify the layout, see https://jekyllrb.com/docs/themes/#overriding-theme-defaults

layout: default
---

# site.github ussage

## Versions

jekyll: {{ site.github.versions.jekyll }}
kramdown: {{ site.github.versions.kramdown }}
liquid: {{ site.github.versions.liquid }}
maruku: {{ site.github.versions.maruku }}
rdiscount: {{ site.github.versions.rdiscount }}
redcarpet: {{ site.github.versions.redcarpet }}
RedCloth: {{ site.github.versions.RedCloth }}
jekyll-mentions: {{ site.github.versions.jekyll-mentions }}
jekyll-redirect-from: {{ site.github.versions.jekyll-redirect-from }}
jekyll-sitemap: {{ site.github.versions.jekyll-sitemap }}
github-pages: {{ site.github.versions.github-pages }}
ruby: {{ site.github.versions.ruby }}

## hostname

{{ site.github.hostname }}

## pages_hostname

{{ site.github.pages_hostname }}

## api_url

[api {{ site.github.api_url }}]({{ site.github.api_url }})

## help_url

[help_url {{ site.github.help_url }}]({{ site.github.help_url }})

## environment

{{ site.github.environment }}

## pages_env

{{ site.github.pages_env }}

## public_repositories

{% for repository in site.github.public_repositories %}
  * [{{ repository.name }}]({{ repository.html_url }})
{% endfor %}

## organization_members

{{ site.github.organization_members }}

## build_revision

{{ site.github.build_revision }}

## project_title

{{ site.github.project_title }}

## project_tagline

{{ site.github.project_tagline }}

## owner

  - ![owner_gravatar_url]({{ site.github.owner_gravatar_url }})
  - [{{ site.github.owner_name }}]({{ site.github.owner_url }})

## repository

[Name: {{ site.github.repository_name }}, URL: {{ site.github.repository_url }}]({{ site.github.repository_url }})

repository_nwo: {{ site.github.repository_nwo }}

  - [Zip: {{ site.github.zip_url }}]({{ site.github.zip_url }})
  - [Tar: {{ site.github.tar_url }}]({{ site.github.tar_url }})
  - [Clone: {{ site.github.clone_url }}]({{ site.github.clone_url }})
  - [Releases: {{ site.github.releases_url }}]({{ site.github.releases_url }})
  - [Issues: {{ site.github.issues_url }}]({{ site.github.issues_url }})
  - [wiki: {{ site.github.wiki_url }}]({{ site.github.wiki_url }})

## language

{{ site.github.language }}

## is_user_page

{{ site.github.is_user_page }}

## is_project_page

{{ site.github.is_project_page }}

## show_downloads

{{ site.github.show_downloads }}

## url

  - [URL: {{ site.github.url }}]({{ site.github.url }})
  - baseurl: {{ site.github.baseurl }}

## contributors

{% for contributor in site.github.contributors %}
  * {{ contributor.login }}
    - id: {{ contributor.id }}
	- node_id: {{ contributor.node_id }}
    - avatar_url: {{ contributor.avatar_url }}
    - gravatar_id: {{ contributor.gravatar_id }}
    - [url: {{ contributor.url }}]({{ contributor.url }})
    - [html_url: {{ contributor.html_url }}]({{ contributor.html_url }})
    - [followers_url: {{ contributor.followers_url }}]({{ contributor.followers_url }})
    - [following_url: {{ contributor.following_url }}]({{ contributor.following_url }})
    - [gists_url: {{ contributor.gists_url }}]({{ contributor.gists_url }})
    - [starred_url: {{ contributor.starred_url }}]({{ contributor.starred_url }})
    - [subscriptions_url: {{ contributor.subscriptions_url }}]({{ contributor.subscriptions_url }})
    - [organizations_url: {{ contributor.organizations_url }}]({{ contributor.organizations_url }})
    - [repos_url: {{ contributor.repos_url }}]({{ contributor.repos_url }})
    - [events_url: {{ contributor.events_url }}]({{ contributor.events_url }})
    - [received_events_url: {{ contributor.received_events_url }}]({{ contributor.received_events_url }})
    - type: {{ contributor.type }}
    - site_admin: {{ contributor.site_admin }}
    - contributions: {{ contributor.contributions }}
{% endfor %}

## releases

{% for release in site.github.releases %}
  - [url: {{ release.url }}]({{ release.url }})
    - [assets_url: {{ release.assets_url }}]({{ release.assets_url }})
    - [upload_url: {{ release.upload_url }}]({{ release.upload_url }})
    - [html_url: {{ release.html_url }}]({{ release.html_url }})
    - id: {{ release.id }}
	- author: {{ release.author.login }}
	  - id: {{ release.author.id }}
	  - node_id: {{ release.author.node_id }}
      - avatar_url: {{ release.author.avatar_url }}
      - gravatar_id: {{ release.author.gravatar_id }}
      - [url: {{ release.author.url }}]({{ release.author.url }})
      - [html_url: {{ release.author.html_url }}]({{ release.author.html_url }})
      - [followers_url: {{ release.author.followers_url }}]({{ release.author.followers_url }})
      - [following_url: {{ release.author.following_url }}]({{ release.author.following_url }})
      - [gists_url: {{ release.author.gists_url }}]({{ release.author.gists_url }})
      - [starred_url: {{ release.author.starred_url }}]({{ release.author.starred_url }})
      - [subscriptions_url: {{ release.author.subscriptions_url }}]({{ release.author.subscriptions_url }})
      - [organizations_url: {{ release.author.organizations_url }}]({{ release.author.organizations_url }})
      - [repos_url: {{ release.author.repos_url }}]({{ release.author.repos_url }})
      - [events_url: {{ release.author.events_url }}]({{ release.author.events_url }})
      - [received_events_url: {{ release.author.received_events_url }}]({{ release.author.received_events_url }})
      - type: {{ release.author.type }}
      - site_admin: {{ release.author.site_admin }}
      - contributions: {{ release.author.contributions }}
    - node_id: {{ release.node_id }}
    - tag_name: {{ release.tag_name }}
    - target_commitish: {{ release.target_commitish }}
    - name: {{ release.name }}
    - draft: {{ release.draft }}
    - prerelease: {{ release.prerelease }}
    - created_at: {{ release.created_at }}
    - published_at: {{ release.published_at }}
    - assets: {{ release.assets }}
    - [Tar: {{ release.tarball_url }}]({{ release.tarball_url }})
	- [Zip: {{ release.zipball_url }}]({{ release.zipall_url }})
    - body: {{ release.body }}
{% endfor %}


## latest_release

{% for latest_release in site.github.latest_release %}
  * [url: {{ latest_release.url }}]({{ latest_release.url }})
    - [assets_url: {{ latest_release.assets_url }}]({{ latest_release.assets_url }})
    - [upload_url: {{ latest_release.upload_url }}]({{ latest_release.upload_url }})
    - [html_url: {{ latest_release.html_url }}]({{ latest_release.html_url }})
    - id: {{ latest_release.id }}
	- author: {{ latest_release.author.login }}
	  - id: {{ latest_release.author.id }}
	  - node_id: {{ latest_release.author.node_id }}
      - avatar_url: {{ latest_release.author.avatar_url }}
      - gravatar_id: {{ latest_release.author.gravatar_id }}
      - [url: {{ latest_release.author.url }}]({{ latest_release.author.url }})
      - [html_url: {{ latest_release.author.html_url }}]({{ latest_release.author.html_url }})
      - [followers_url: {{ latest_release.author.followers_url }}]({{ latest_release.author.followers_url }})
      - [following_url: {{ latest_release.author.following_url }}]({{ latest_release.author.following_url }})
      - [gists_url: {{ latest_release.author.gists_url }}]({{ latest_release.author.gists_url }})
      - [starred_url: {{ latest_release.author.starred_url }}]({{ latest_release.author.starred_url }})
      - [subscriptions_url: {{ latest_release.author.subscriptions_url }}]({{ latest_release.author.subscriptions_url }})
      - [organizations_url: {{ latest_release.author.organizations_url }}]({{ latest_release.author.organizations_url }})
      - [repos_url: {{ latest_release.author.repos_url }}]({{ latest_release.author.repos_url }})
      - [events_url: {{ latest_release.author.events_url }}]({{ latest_release.author.events_url }})
      - [received_events_url: {{ latest_release.author.received_events_url }}]({{ latest_release.author.received_events_url }})
      - type: {{ latest_release.author.type }}
      - site_admin: {{ latest_release.author.site_admin }}
      - contributions: {{ latest_release.author.contributions }}
    - node_id: {{ latest_release.node_id }}
    - tag_name: {{ latest_release.tag_name }}
    - target_commitish: {{ latest_release.target_commitish }}
    - name: {{ latest_release.name }}
    - draft: {{ latest_release.draft }}
    - prerelease: {{ latest_release.prerelease }}
    - created_at: {{ latest_release.created_at }}
    - published_at: {{ latest_release.published_at }}
    - assets: {{ latest_release.assets }}
    - [Tar: {{ latest_release.tarball_url }}]({{ latest_release.tarball_url }})
	- [Zip: {{ latest_release.zipball_url }}]({{ latest_release.zipball_url }})
    - body: {{ release.body }}
{% endfor %}

## private

{{ site.github.private }}

## archived

{{ site.github.archived }}

## disabled

{{ site.github.disabled }}

## license

  - key: {{ site.github.license.key }}
  - name: {{ site.github.license.name }}
  - spdx_id: {{ site.github.license.spdx_id }}
  - key: {{ site.github.license.key }}
  - [url: {{ site.github.license.url }}]({{ site.github.license.url }})

## source

  - branch: {{ site.github.source.branch }}
  - path: {{ site.github.source.path }}
  
This site is open source. {% github_edit_link "Improve this page" %}

<p>This site is open source. {% raw %}{% github_edit_link "Improve this page" %}{% endraw %}</p>

<p>This site is open source. <a href="{% raw %}{% github_edit_link %}{% endraw %}">Improve this page</a></p>
