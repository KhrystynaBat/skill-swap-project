import { expect, type Page, test } from '@playwright/test';

const tokenForUser = (userId: number) => {
  const payload = {
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': String(userId),
  };

  return `header.${Buffer.from(JSON.stringify(payload)).toString('base64')}.signature`;
};

async function mockProfileApi(page: Page) {
  await page.route('**/api/profile/me', async (route) => {
    await route.fulfill({
      json: {
        id: 1,
        name: 'Olesia',
        email: 'olesia@test.com',
        city: 'Lviv',
        bio: 'Student',
      },
    });
  });

  await page.route('**/api/profile/skills', async (route) => {
    await route.fulfill({ json: [] });
  });

  await page.route('**/api/profile/interests', async (route) => {
    await route.fulfill({ json: [] });
  });
}

async function loginInBrowser(page: Page, userId = 1) {
  await page.addInitScript((token) => {
    window.sessionStorage.setItem('skill_swap_token', token);
  }, tokenForUser(userId));
}

test('login page submits credentials and opens profile', async ({ page }) => {
  await mockProfileApi(page);
  await page.route('**/api/auth/login', async (route) => {
    await route.fulfill({ json: { token: tokenForUser(1) } });
  });

  await page.goto('/login');
  await page.getByLabel('Email').fill('olesia@test.com');
  await page.getByLabel('Password').fill('Password1');
  await page.locator('form').getByRole('button', { name: 'Login' }).click();

  await expect(page).toHaveURL(/\/profile$/);
});

test('register page creates account, logs in and opens profile', async ({ page }) => {
  await mockProfileApi(page);
  await page.route('**/api/auth/register', async (route) => {
    await route.fulfill({ body: 'User registered' });
  });
  await page.route('**/api/auth/login', async (route) => {
    await route.fulfill({ json: { token: tokenForUser(1) } });
  });

  await page.goto('/register');
  await page.getByLabel('Name').fill('Olesia Baidala');
  await page.getByLabel('Email').fill('olesia@test.com');
  await page.getByLabel('Password').fill('Password1');
  await page.locator('form').getByRole('button', { name: 'Register' }).click();

  await expect(page).toHaveURL(/\/profile$/);
});

test('search page finds user and creates match from the UI', async ({ page }) => {
  await loginInBrowser(page);
  await page.route('**/api/skills', async (route) => {
    await route.fulfill({
      json: [
        { id: 1, name: 'C#', category: 'IT' },
        { id: 2, name: 'Photography', category: 'Art' },
      ],
    });
  });
  await page.route('**/api/users/search**', async (route) => {
    await route.fulfill({
      json: [
        {
          id: 2,
          name: 'Khrystyna',
          city: 'Lviv',
          teachSkills: [{ name: 'Photography', category: 'Art', level: 5 }],
          learnSkills: [{ name: 'C#', category: 'IT', priority: 4 }],
        },
      ],
    });
  });
  await page.route('**/api/match/2', async (route) => {
    await route.fulfill({ body: 'Match created' });
  });

  await page.goto('/users/search');
  await page.getByLabel('Search skill').fill('Photo');
  await page.getByRole('button', { name: /Photography/ }).click();
  await page.locator('form').getByRole('button', { name: 'Search' }).click();
  await page.getByRole('button', { name: 'Match' }).click();

  await expect(page).toHaveURL(/\/matches$/);
});

test('matches page finishes active match with review form', async ({ page }) => {
  await loginInBrowser(page);
  let status = 'active';

  await page.route('**/api/match/my', async (route) => {
    await route.fulfill({
      json: [{ id: 7, userAId: 1, userBId: 2, status }],
    });
  });
  await page.route('**/api/users/2', async (route) => {
    await route.fulfill({
      json: { id: 2, name: 'Khrystyna', city: 'Lviv' },
    });
  });
  await page.route('**/api/review/user/2', async (route) => {
    const body = route.request().postDataJSON();
    expect(body).toEqual({ rating: 5, comment: 'Great job!' });
    status = 'completed';
    await route.fulfill({ body: 'Review created' });
  });

  await page.goto('/matches');
  await page.getByRole('button', { name: 'Finish match' }).click();
  await page.getByLabel('Comment').fill('Great job!');
  await page.getByRole('button', { name: 'Save review' }).click();

  await expect(page.getByText('Completed')).toBeVisible();
});
