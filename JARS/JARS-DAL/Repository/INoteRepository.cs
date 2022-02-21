using JARS_DAL.Models;
namespace JARS_DAL.Repository;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetNotes();
    Task<Note?> GetNote(int id);
    Task Delete(Note note);
    Task Add(Note note);
    Task Update(Note note);
    Task<Note?> GetNoteByTransactionId(int? transactionId);
    Task<Note?> GetNoteByContractId(int? contractId);
}