# Sample Data Password Reference

All passwords in `04_sample_data.sql` are **SHA256 hashed**. Use the plain-text passwords below for login testing:

## Admins
| Username | Password | Email |
|----------|----------|-------|
| alice_smith | admin123 | alice.smith@email.com |
| jessica_lopez | admin456 | jessica.lopez@email.com |

## Gym Owners
| Username | Password | Email |
|----------|----------|-------|
| eva_brown | owner123 | eva.brown@email.com |
| james_williams | owner456 | james.williams@email.com |
| sophia_martinez | owner789 | sophia.martinez@email.com |

## Trainers
| Username | Password | Email |
|----------|----------|-------|
| bob_johnson | trainer123 | bob.johnson@email.com |
| zara_garcia | trainer456 | zara.garcia@email.com |
| emily_anderson | trainer789 | emily.anderson@email.com |
| daniel_gonzalez | trainer101 | daniel.gonzalez@email.com |
| andrew_young | trainer102 | andrew.young@email.com |

## Members
| Username | Password | Email |
|----------|----------|-------|
| john_doe | member123 | john.doe@email.com |
| jane_doe | member456 | jane.doe@email.com |
| michael_lee | member789 | michael.lee@email.com |
| sarah_wilson | member101 | sarah.wilson@email.com |
| kevin_hernandez | member102 | kevin.hernandez@email.com |
| amanda_white | member103 | amanda.white@email.com |
| ryan_clark | member104 | ryan.clark@email.com |
| nicole_flores | member105 | nicole.flores@email.com |
| tyler_garcia | member106 | tyler.garcia@email.com |
| hannah_gonzalez | member107 | hannah.gonzalez@email.com |

---

**Note**: When logging in, use the plain-text passwords above. The application will automatically hash them before sending to the database.

**Database Storage**: The `Users` table stores passwords as SHA256 Base64 hashes (255 characters max).
