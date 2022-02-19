using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
namespace JARS_DAL.DAO;

public class NoteManagement
{
    private static NoteManagement instance = null;
    private static readonly object instanceLock = new object();
    private NoteManagement() { }
    public static NoteManagement Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new NoteManagement();
                }
                return instance;
            }
        }
    }
    public async Task<IEnumerable<Note>> GetNotes()
    {
        try
        {
            var context = new JarsDatabaseContext();
            return await context.Notes.ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<Note?> GetNote(int id)
    {
        try
        {
            var context = new JarsDatabaseContext();
            return await context.Notes.FindAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Note?> GetNoteByContractId(int? contractId)
    {
        try
        {
            var context = new JarsDatabaseContext();
            return await context.Notes
                .SingleOrDefaultAsync(n => n.ContractId == contractId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Note?> GetNoteByTransactionId(int? transactionId)
    {
        try
        {
            var context = new JarsDatabaseContext();
            return await context.Notes
                .SingleOrDefaultAsync(n => n.TransactionId == transactionId);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task Add(Note note)
    {
        try
        {
            var context = new JarsDatabaseContext();
            context.Notes.Add(note);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task Update(Note note)
    {
        try
        {
            var context = new JarsDatabaseContext();
            context.Update(note);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task Delete(Note note)
    {
        try
        {
            Note? _note = await GetNote(note.Id);
            var context = new JarsDatabaseContext();
            context.Notes.Remove(note);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}