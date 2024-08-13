---
title: Coreframework API
---

## put content here.


{% for my_page in site.pages %}
  {%- if my_page.title -%}
    <p><a class="page-link" href="{{ my_page.url | relative_url }}">{{ my_page.title | escape }}</a></p>
  {%- endif -%}
{% endfor %}