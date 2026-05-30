# Job Portal Design Specifications

## 1. Global Theming
- **Primary Color:** `#4640DE` (Used for buttons, links, and active states)
- **Background Color (Light):** `#F7F5FC` (Used for page headers, hero sections, and highlighted areas)
- **Background Color (White):** `#FFFFFF` (Used for main content areas and cards)
- **Text Color (Dark):** `#25324B` (Used for headings and primary text)
- **Text Color (Muted):** `#515B6F` (Used for paragraphs and secondary text)
- **Border Color:** `#D6DDEB` (Used for dividers, borders, and card outlines)
- **Font Family:** `Inter`, sans-serif (Global font for all typography)

## 2. Typography Hierarchy
- **H1 (Hero):** 72px (md), 5xl (mobile), Bold, Dark, leading tight.
- **H1 (Page Headers):** 40px (md), 32px (mobile), Bold, Dark.
- **H2 (Section Titles):** 32px, Bold, Dark.
- **H3 (Card Titles):** 24px, Bold, Dark.
- **Body Text:** 16px to 18px, Muted, leading relaxed.
- **Small/Secondary Text:** 14px, Muted.

## 3. UI Components

### Cards (Jobs, Dashboard, Profile, Info)
- **Container:** White background (`bg-white`), heavily rounded corners (`rounded-[20px]`).
- **Shadow:** Standardized soft drop shadow across all primary cards `shadow-[0_8px_30px_rgb(0,0,0,0.12)]`.
- **Hover Effects:** For interactive cards (e.g., job listings, dashboard stats), add `transition-transform hover:-translate-y-1`, `hover:border-blue-600`, or `hover:shadow-md`.

### Search Bar
- **Shape:** Fully rounded (`rounded-full` on desktop, `rounded-3xl` on mobile).
- **Shadow:** Soft shadow `shadow-sm` or `shadow-[0_15px_40px_rgba(70,64,222,0.08)]`.
- **Button:** Fully rounded (`rounded-full`), Primary background, White text, Bold.
- **Inputs:** No outline on focus, text-dark, placeholder-gray-400, truncate.

### Background Patterns
- **SVG Lines:** Used in `#F7F5FC` sections. Features large overlapping abstract geometric paths with `#C4C4E6` stroke, `opacity-70`. Placed absolutely to the top right of the container, avoiding overlap with crucial content.

### Logos
- **Header Logo:** 40x40px (`w-10 h-10`), primary background circle with white geometric shapes. Text size `28px`.
- **Company Profile Logo:** 160x160px (`w-40 h-40`), white background, bordered, shadow.

### Filters Sidebar
- **Container:** Wrapped in a border (`border-bordercolor`), rounded corners (`rounded-2xl`), padding `p-6`.
- **Checkboxes:** Primary color on check, rounded.
- **Text:** Muted by default, transitioning to dark on hover.

## 4. Pages Structure

### Home Page (`Home/Index.cshtml`)
- **Hero Section:** Light background `#F7F5FC` with SVG pattern. Features large text, call to action, and search bar.
- **Categories & Jobs:** Grid layout, white background, cards with hover effects.

### Job List Page (`Jobs/Index.cshtml`)
- **Header:** `#F7F5FC` background, breadcrumb, page title, and rounded search bar. SVG pattern integrated.
- **Layout:** 2-column layout (1/4 sidebar for filters with borders, 3/4 main content for job cards).

### Job Details Page (`Jobs/Details.cshtml`)
- **Header:** `#F7F5FC` background, SVG pattern, breadcrumb.
- **Main Content:** 2-column layout. Left column for job description (white card, bordered, padded). Right column for company summary and similar jobs.

### Company Details Page (`Companies/Details.cshtml`)
- **Header:** `#F7F5FC` background, SVG pattern, breadcrumb. Includes overlapping large company logo. Location anchored (e.g., "TP Hà Nội").
- **Layout:** Sidebar navigation (tabs) for "Open positions", "About", "Contact". Main content area for tab details.

## 5. Implementation Rules
- **Tailwind CSS:** Rely entirely on Tailwind classes for styling. Do not write custom CSS unless absolutely necessary.
- **Responsiveness:** Always design mobile-first. Use `md:`, `lg:` prefixes to adjust layout for larger screens (e.g., stacking columns on mobile, side-by-side on desktop).
- **Consistency:** If a new component is introduced, ensure it reuses the established color palette and border radii conventions.
